using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBook.FirebaseConfig
{
    public static class FirebaseConfig
    {
        public static readonly string ProjectId = "recipebook-e2874";
        public static readonly string ApiKey = "AIzaSyBRLxAAT7_j2cjY58LrclZ3z9bUo9OU8_c";
        public static readonly string FirestoreBaseUrl = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
        public static readonly string StorageBaseUrl = $"https://firebasestorage.googleapis.com/v0/b/{ProjectId}.appspot.com/o";

        public static HttpClient GetHttpClient()
        {
            return new HttpClient();
        }
    }

}
