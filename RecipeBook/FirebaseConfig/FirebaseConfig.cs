using System.Net.Http;

namespace RecipeBook.FirebaseConfig
{
    public static class FirebaseConfig
    {
        public static readonly string ProjectId = "recipebook-e2874";
        public static readonly string ApiKey = "AIzaSyBRLxAAT7_j2cjY58LrclZ3z9bUo9OU8_c";
        public static readonly string FirestoreBaseUrl = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";
        public static readonly string StorageBaseUrl = $"https://firebasestorage.googleapis.com/v0/b/{ProjectId}.appspot.com/o";

        // Добавлен URL для Realtime Database
        public static readonly string RealtimeDatabaseUrl = $"https://{ProjectId}-default-rtdb.europe-west1.firebasedatabase.app/";

        public static HttpClient GetHttpClient()
        {
            return new HttpClient();
        }
    }
}
