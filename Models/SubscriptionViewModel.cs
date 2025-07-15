using HarmonySound.Models;
namespace HarmonySound.MVC.Models
{
    public class SubscriptionViewModel
    {
        public Plan? CurrentPlan { get; set; }
        public List<Plan> PremiumPlans { get; set; }
    }
}
