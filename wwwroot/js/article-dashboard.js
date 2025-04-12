

//hiện ds article của bác sĩ
async function loadArticleList() {

    //lấy status từ attribute data-status của button
    const status = document.getElementById("table-article").getAttribute("data-status");
    const keyword = document.getElementById("searchInput").value || "";
    console.log("status", status);
  try {
    const res = await fetch(`/Article/DoctorArticles?status=${status}&keyword=${keyword}`);
    const data = await res.json();
    console.log(data);

    document.getElementById("article-list").innerHTML = data.data
      .map(
        (article) => `<tr>
                                <td>
                                    <img src='${
                                      article.Image
                                    }' class="thumbnail-img" alt="Thumbnail">
                                </td>
                                <td>
                                    <div class="article-title">${
                                      article.Title
                                    }</div>
                                </td>
                                <td><span class="category-badge">${
                                  article.Category.Name
                                }</span></td>
                                <td><span class="badge ${
                                  article.Status == "published"
                                    ? "badge-published"
                                    : "badge-draft"
                                }">${
          article.Status == "published" ? "Đã đăng" : "Nháp"
        }</span></td>
                                <td>${article.CreatedAt}</td>
                                <td>
                                    <a href="/Article/Update/${article.Id}" class="action-btn btn-edit" title="Sửa"><i class="fas fa-edit"></i></a>
                                    <button class="action-btn btn-delete" title="Xóa" onclick="showDeleteArticleModal(${article.Id})"><i
                                            class="fas fa-trash"></i></button>
                                </td>
                            </tr> `
      )
      .join(" ");
  } catch (error) {
    console.error("Error loading categories:", error);
    return [];
  }
}
loadArticleList();

//xu ly xoa article
let articleIdToDelete = null;//biến toàn cục để lưu id của article cần xóa
//show modal va set lai id xóa article
function showDeleteArticleModal(id) {
  articleIdToDelete = id;
  //hiện modal xóa article
   const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
   deleteModal.show();
}
async function deleteArticle() {
  try {
    const res = await fetch(`/Article/Delete/${articleIdToDelete}`, {
      method: "DELETE",
    });
    const data = await res.json();
    console.log(data);
    if (data.success) {
      loadArticleList();//load lại danh sách bài viết
    } else {
      alert("Có lỗi xảy ra!");
    }
  } catch (error) {
    console.error("Error deleting article:", error);
  }
  //đóng modal xóa article
  const deleteModal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
  deleteModal.hide();
}

