
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginUMB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FactsView : ContentView
    {
        private readonly Dictionary<string, List<string>> _userFavorites;
        private string _userName;
        private List<string> Favorites => _userFavorites[_userName];
        private readonly RestClient _client = new RestClient("https://api.chucknorris.io");

        public FactsView()
        {
            InitializeComponent();

            var categoryList = new List<string> { "Random" };
            categoryList.AddRange(GetCategories());
            CategoryPicker.ItemsSource = categoryList;
            CategoryPicker.SelectedIndex = 0;
            _userFavorites = new Dictionary<string, List<string>>();
        }

        private void GetFavoriteClicked(object sender, EventArgs e)
        {
            FactLabel.Text = Favorites.Count == 0
                ? "Tu no tienes favoritos aún."
                : Favorites[new Random().Next(0, Favorites.Count)];
        }

        private void GetFactClicked(object sender, EventArgs e)
        {
            var isRandom = CategoryPicker.SelectedIndex == 0;
            FactLabel.Text = GetFact(isRandom ? null : CategoryPicker.SelectedItem.ToString());
        }

        public string GetFact(string category = null)
        {
            var url = "/jokes/random";
            if (category != null) url += "?category=" + category;
            var fact = Get<Fact>(url);
            return fact?.value;
        }

        public string[] GetCategories()
        {
            return Get<string[]>("/jokes/categories");
        }

        private T Get<T>(string url)
        {
            var request = new RestRequest(url, Method.GET);
            var response = _client.Execute(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private void AddFavoriteClicked(object sender, EventArgs e)
        {
            Favorites.Add(FactLabel.Text);
        }
    }
}