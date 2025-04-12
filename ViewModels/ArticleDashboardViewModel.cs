using System.ComponentModel.DataAnnotations;

namespace WebsiteTuVan.ViewModels
{
    public class ArticleDashboardViewModel
    {
       public string type { get; set; } = "all";
       public List<ArticleViewModel> articleList { get; set; } = new List<ArticleViewModel>();
    }
}