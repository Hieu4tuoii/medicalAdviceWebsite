async function loadTopDoctorList() {
  const response = await fetch("/Doctor/GetTop?size=4");
  if (response.ok) {
    const data = await response.json();
    const doctorList = document.getElementById("doctor-list");
    console.log(data);
    doctorList.innerHTML = data.data
      .map(
        (doctor) =>
          `
        <div class="col-6 col-md-4 col-lg-3">
                    <div class="doctor-card">
                        <!-- Container mới cho ảnh -->
                        <div class="doctor-image-container">
                            <img src="${doctor.Image}" alt="Ảnh Bác Sĩ"
                                class="doctor-avatar">
                        </div>
                        <div class="doctor-info text-center">
                            <h5 class="doctor-name mb-0">BS. ${doctor.User.Name}</h5>
                        </div>
                    </div>
                </div>
        `
      )
      .join(" ");
  } else {
    console.error("Failed to load top doctors");
  }
}
loadTopDoctorList();

//xử lý load 8 bài viết mới nhất
async function loadTopArticleList() {
  const response = await fetch("/Article/Published?size=8");
  if (response.ok) {
    const data = await response.json();
    const doctorList = document.getElementById("post-list");
    console.log(data);
    doctorList.innerHTML = data.data
      .map(
        (article) =>
          `
          <div class="col-6 col-md-4 col-lg-3">
                    <div class="post-card">
                        <a href="/Article/Detail/${article.Id}" class="post-card-link"> <!-- Link bao cả thẻ card -->
                            <div class="post-image-container">
                                <img src="${article.Image}"
                                    class="post-thumbnail">
                            </div>
                            <div class="post-info text-center">
                                <h5 class="post-title mb-0">${article.Title}</h5>
                            </div>
                        </a>
                    </div>
                </div>
          `
      )
      .join(" ");
  } else {
    console.error("Failed to load top doctors");
  }
}
loadTopArticleList();
