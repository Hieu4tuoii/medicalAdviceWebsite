async function loadAllDoctor() {
    const response = await fetch("/Doctor/GetAll");
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
  loadAllDoctor();