//hiện ds select category
async function loadAddCategory() {
  try {
    const res = await fetch("/Category");
    const data = await res.json();
    console.log(data);

    document.getElementById("form-select-category").innerHTML = data.data
      .map(
        (category) => `<option value="${category.Id}">${category.Name}</option>`
      )
      .join(" ");
  } catch (error) {
    console.error("Error loading categories:", error);
    return [];
  }
}
loadAddCategory();

// Thêm xử lý cho việc upload ảnh
document.addEventListener("DOMContentLoaded", function() {
  const imageInput = document.getElementById("image");
  const imagePreview = document.getElementById("imagePreview");
  const imageUrlInput = document.getElementById("imageUrl");

  // Hiển thị ảnh preview khi chọn file
  imageInput.addEventListener("change", async function() {
    if (this.files && this.files[0]) {
      const file = this.files[0];
      
      // Kiểm tra kích thước file (tối đa 5MB)
      if (file.size > 5 * 1024 * 1024) {
        alert("Kích thước file quá lớn. Tối đa 5MB được cho phép.");
        this.value = "";
        return;
      }

      // Hiển thị preview
      const reader = new FileReader();
      reader.onload = function(e) {
        imagePreview.src = e.target.result;
        imagePreview.style.display = "block";
      }
      reader.readAsDataURL(file);

      // Upload file
      try {
        const formData = new FormData();
        formData.append("file", file);
        
        const response = await fetch("/Upload/image", {
          method: "POST",
          body: formData
        });

        if (response.ok) {
          const result = await response.json();
          if (result.success) {
            imageUrlInput.value = result.data;
            console.log("Ảnh đã được upload:", result.data);
          } else {
            alert("Upload ảnh thất bại: " + result.message);
          }
        } else {
          alert("Upload ảnh thất bại");
        }
      } catch (error) {
        console.error("Lỗi khi upload ảnh:", error);
        alert("Có lỗi xảy ra khi upload ảnh");
      }
    }
  });
});

//thêm bài viết
async function saveArticle(status) {
  const title = document.getElementById("title").value;
  const content = document.getElementById("content").value;
  const categoryId = document.getElementById("form-select-category").value;
  const imageUrl = document.getElementById("imageUrl").value;

  const data = {
    Title: title,
    Content: content,
    CategoryId: parseInt(categoryId),
    Image: imageUrl, 
    Status: status,
  };
  console.log(data);
  try {
    const res = await fetch("/Article/Add", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    const result = await res.json();
    console.log(result);
    if (result.success) {
      alert("Đã thêm bài viết thành công!");
      window.location.href = "/Article/Dashboard/all"; // Chuyển hướng về trang dashboard
    } else {
      alert(result.message);
    }
  } catch (error) {
    console.error("Error adding article:", error);
  }
}

