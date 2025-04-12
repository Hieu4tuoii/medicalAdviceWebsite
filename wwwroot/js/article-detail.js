//get article by id
async function loadArticleById() {
  const articleId = document
    .getElementById("article-container")
    .getAttribute("article-id");
  try {
    const res = await fetch(`/Article/${articleId}`);
    const data = await res.json();
    console.log(data);
    if (data.success) {
      document.getElementById("title").innerHTML = data.data.Title;
      document.getElementById("category").innerHTML = data.data.Category.Name;
      document.getElementById("content").innerHTML = data.data.Content;
      document.getElementById("image").setAttribute("src", data.data.Image);

      loadDoctors();
      //load ds bài viết liên quan
      loadRelatedArticles(data.data.Category.Id);
    } else {
      alert(data.message);
    }
  } catch (error) {
    console.error("Error loading article:", error);
  }
}
loadArticleById();

// Tải danh sách bác sĩ
async function loadDoctors() {
  try {
    const res = await fetch("/Doctor/GetAll");
    const data = await res.json();

    //nếu thành công thì hiện 5 bác sĩ đầu tiên
    if (data.success && data.data.length > 0) {
      const doctorsHtml = data.data
        .slice(0, 5)
        .map(
          (doctor) => `
          <li>
            <a href="#" class="doctor-item">
              <div class="doctor-avatar">
                <img src="${
                  doctor.User.Avatar || "/images/default-avatar.png"
                }" alt="${doctor.User.Name}">
              </div>
              <div class="doctor-details">
                <p class="doctor-name">BS. ${doctor.User.Name}</p>
                <p class="doctor-specialty">${doctor.Specialization}</p>
              </div>
            </a>
          </li>
        `
        )
        .join("");

      document.getElementById("doctors-list").innerHTML = doctorsHtml;
    } else {
      document.getElementById("doctors-list").innerHTML =
        "<li>Không có bác sĩ nào.</li>";
    }
  } catch (error) {
    console.error("Error loading doctors:", error);
    document.getElementById("doctors-list").innerHTML =
      "<li>Lỗi tải danh sách bác sĩ.</li>";
  }
}

//load bài viết có cùng thể loại
async function loadRelatedArticles(categoryId) {
  try {
    const res = await fetch(
      `/Article/Published?categoryId=${categoryId}`
    );
    const data = await res.json();

    if (data.success && data.data.length > 0) {
      const relatedHtml = data.data
        .slice(0, 4) // Chỉ lấy 5 bài viết liên quan
        .map(
          (article) => `
          <div class="related-item">
            <a href="/Article/Detail/${article.Id}">
              <div class="related-image">
                <img src="${article.Image}" alt="${article.Title}">
              </div>
              <div class="related-info">
                <h5>${article.Title}</h5>
                <p>${article.Category.Name}</p>
              </div>
            </a>
          </div>
        `
        )
        .join("");
      document.getElementById("related-posts").innerHTML = relatedHtml;
    } else {
      document.getElementById("related-posts").innerHTML =
        "<p>Không có bài viết liên quan.</p>";
    }
  } catch (error) {
    console.error("Error loading related articles:", error);
  }
}
