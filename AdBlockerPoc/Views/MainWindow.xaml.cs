using Microsoft.Web.WebView2.Core;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using AdBlockerPoc.Models;
using AdBlockerPoc.Services;

namespace AdBlockerPoc
{
    public partial class MainWindow : Window
    {
        private const string InitialUrl = "https://www.weather.com";
        private const int TimerIntervalMilliseconds = 200;

        private readonly ObservableCollection<Item> _items;
        private readonly ConcurrentQueue<Item> _itemQueue;
        private bool _isAdBlockerEnabled = true;

        // TODO: Use https://easylist.to/ instead of hardcoding ad domains
        private readonly HashSet<string> _adDomains = new()
        {
            "doubleclick.net", "googlesyndication.com", "adnxs.com", "adsafeprotected.com",
            "advertising.com", "rubiconproject.com", "openx.net", "yieldmanager.com",
            "media.net", "adform.net", "adroll.com", "outbrain.com", "taboola.com",
            "criteo.com", "revcontent.com", "zedo.com", "popads.net", "adblade.com",
            "adf.ly", "infolinks.com"
        };

        public MainWindow()
        {
            InitializeComponent();

            _items = new ObservableCollection<Item>();
            _itemQueue = new ConcurrentQueue<Item>();
            ItemsListBox.ItemsSource = _items;

            var batchProcessor = new BatchProcessor(_items, _itemQueue, TimerIntervalMilliseconds, OnItemProcessed);
            batchProcessor.Start();

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await WebView.EnsureCoreWebView2Async(null);

            WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

            var webResourceRequestedObservable = Observable.FromEventPattern<CoreWebView2WebResourceRequestedEventArgs>(
                WebView.CoreWebView2, nameof(WebView.CoreWebView2.WebResourceRequested))
                .Select(evt => evt.EventArgs);

            webResourceRequestedObservable
                .Subscribe(webResourceRequestedEventArgs =>
                {
                    var uri = new Uri(webResourceRequestedEventArgs.Request.Uri);
                    var host = uri.Host;

                    bool isAd = _adDomains.Contains(host) || _adDomains.Any(domain => host.EndsWith($".{domain}"));

                    if (isAd)
                    {
                        var newItem = new Item(uri.AbsoluteUri, _isAdBlockerEnabled);
                        _itemQueue.Enqueue(newItem);

                        if (_isAdBlockerEnabled)
                        {
                            var response = WebView.CoreWebView2.Environment.CreateWebResourceResponse(
                                new MemoryStream(),
                                403,
                                "Blocked",
                                "Content-Type: text/plain"
                            );

                            webResourceRequestedEventArgs.Response = response;
                        }
                    }
                });

            WebView.CoreWebView2.Navigate(InitialUrl);
        }

        private void OnItemProcessed(Item newItem)
        {
            ItemsListBox.ScrollIntoView(newItem);
            ItemsListBox.SelectedItem = newItem;
        }

        private void AdBlockerButton_Click(object sender, RoutedEventArgs e)
        {
            _isAdBlockerEnabled = !_isAdBlockerEnabled;
            AdBlockerButton.Content = _isAdBlockerEnabled ? "Disable Ad Blocker" : "Enable Ad Blocker";
            ReloadPage();
        }

        private void ReloadPage()
        {
            _items.Clear();
            while (_itemQueue.TryDequeue(out _)) { }
            WebView.CoreWebView2.Navigate(InitialUrl);
        }
    }
}
