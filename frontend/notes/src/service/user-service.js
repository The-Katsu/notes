const _apiBase = "https://localhost:7093/";
var token = "";

class UserService {

  async Authorize(username, password) {
    const res = await fetch(`${_apiBase}login/?username=${username}&password=${password}`);
    if(!res.ok) {

    }
    return await res.text();
  }
}