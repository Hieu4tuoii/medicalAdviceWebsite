//get published articles
async function loadPublishedArticles() {
  try {
    const res = await fetch("/Article/Published");
    const data = await res.json();
    console.log(data);

    document.getElementById("article-list").innerHTML = data.data
      .map(
        (article) => `
      <div class="col">
      <a href="/Article/Detail/${article.Id}" class="text-decoration-none">

        <div class="card h-100 article-card">
          <div class="card-img-container">

            <img src="${article.Image}" class="card-img-top article-image" alt="${article.Title}">

          </div>
          <div class="card-body">
            <h5 class="card-title main-title">${article.Title}</h5>
            <p class="card-text subtitle">${article.Category.Name}</p>
          </div>
        </div>
      </a>

      </div>
      `
      )
      .join(" ");
    return data.data;
  } catch (error) {
    console.error("Error loading published articles:", error);
    return [];
  }
}

loadPublishedArticles();
