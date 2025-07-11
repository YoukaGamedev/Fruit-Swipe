mergeInto(LibraryManager.library, {
  SubmitScore: function(score) {
    try {
      var urlParams = new URLSearchParams(window.location.search);
      var gameId = urlParams.get("gameId");

      if (!gameId) {
        console.error("gameId tidak ditemukan di parameter URL.");
        return;
      }

      var apiUrl = "https://ginvostudio.com/portal/game/submit-score/" + gameId;
      var body = JSON.stringify({ score: score });

      fetch(apiUrl, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: body
      }).then(response => {
        if (!response.ok) {
          console.error("SubmitScore gagal: HTTP " + response.status);
        } else {
          console.log("Score berhasil dikirim!");
        }
      }).catch(err => {
        console.error("SubmitScore error:", err);
      });

    } catch (e) {
      console.error("SubmitScore exception:", e);
    }
  },

  ShowLeaderboardModal: function () {
    try {
      // Jika di iframe, gunakan parent.$
      if (typeof parent.parent !== 'undefined' && parent.parent.$) {
        parent.parent.$('#leaderboard').modal('show');
      } 
      else if (typeof parent !== 'undefined' && parent.$) {
        parent.$('#leaderboard').modal('show');
      } 
      else {
        $('#leaderboard').modal('show');
      }
    } catch (e) {
      console.error("ShowLeaderboardModal error:", e);
    }
  }
});