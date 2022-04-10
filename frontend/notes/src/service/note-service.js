const _apiBase = "https://localhost:7093/";

class NotesService {

    async GetResource(url, token) {
      const res = await fetch(`${_apiBase}${url}`, {
          headers: {
              Authorization: `Bearer ${token}`
          }
      })
      if(!res.ok) {
        throw new Error("Ooops! Smth gone wrong!");
      }
      return res.json();
    }

    async PostResource(url, token) {
        const res = await fetch(`${_apiBase}${url}`, {
            method: 'POST',
            headers: {
                Authorization: `Bearer ${token}`
            }
        })
        if(!res.status != 201) {
          throw new Error("Ooops! Smth gone wrong!");
        }
        return res.json();
    }
  
    async GetNote(token) {
        const res = await this.GetResource('', token);
        return res;
    }
  
    async GetAllNotes(id, token) {
        const res = await this.GetResource(`${id}`, token);
        return res;
    }
  
    PostNote() {
        
    }
  
    DeleteNote() {
      
    }
  }
  