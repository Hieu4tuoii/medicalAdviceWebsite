using Microsoft.AspNetCore.Mvc;
using WebsiteTuVan.Repositories;
using WebsiteTuVan.ViewModels;
using WebsiteTuVan.Models;

namespace WebsiteTuVan.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IUsersRepository _repository;
        private readonly IDoctorsRepository _doctorRepository;

        public UserController(IUsersRepository repository, IDoctorsRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
            _repository = repository;
        }
        public async Task<IActionResult> Index()
        {
            var questions = await _repository.GetAllAsync();
            return View(questions);
        }

        //Truy cập trang đăng ký
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        //Truy cập trang đăng nhập
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        //xử lý đăng ký
        [ValidateAntiForgeryToken]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model){
            if(ModelState.IsValid){
                //kiểm tra email đã tồn tại chưa
                if(await _repository.ExistsByEmailAsync(model.Email)){
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(model);
                }
                var user = new User{
                    Name = model.Name,
                    Email = model.Email,
                };
                //set mật khẩu đã mã hóa
                user.SetPassword(model.Password);
                //lưu người dùng vào cơ sở dữ liệu
                await _repository.saveUserAsync(user);
                //sau khi lưu thi chuyển hướng về trang đăng nhập
                return RedirectToAction("Login", "User");
            }
            //nếu dữ liệu không hợp lệ thì trả về view
            return View(model);
        }

        //xử lý đăng nhập
        [ValidateAntiForgeryToken]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model){
            if(ModelState.IsValid){
                //kiểm tra email và mật khẩu
                if(await _repository.AuthenticateAsync(model.Email, model.Password)){
                    //nếu đăng nhập thành công set session cho người dùng va chuyển hướng về trang chủ
                    var user = await _repository.GetUserByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Đã xảy ra lỗi, vui lòng thử lại.");
                        return View(model);
                    }
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Email", user.Email.ToString());
                    HttpContext.Session.SetString("Name", user.Name.ToString());
                    HttpContext.Session.SetString("Role", user.Role.ToString());
                    //nễu là bác sĩ thì lưu thêm cả id bác sĩ vào session
                    if (user.Role == "doctor")
                    {
                        var doctor = await _doctorRepository.GetDoctorByUserIdAsync(user.Id);
                        if (doctor != null)
                        {
                            HttpContext.Session.SetInt32("DoctorId", doctor.Id);
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Địa chỉ email hoặc mật khẩu không đúng");
            }
            return View(model);
        }

        //xử lý đăng xuất
        [HttpGet("Logout")]
        public IActionResult Logout(){
            //xóa session
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}