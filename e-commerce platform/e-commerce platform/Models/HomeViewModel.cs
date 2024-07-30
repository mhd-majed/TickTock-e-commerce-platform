using e_commerce_platform.Models;

namespace e_commerce_platform.ViewModels
{
	public class HomeViewModel
	{
		public List<Product> LatestProducts { get; set; }
		public List<Testimonial> ApprovedTestimonials { get; set; }
	}
}
