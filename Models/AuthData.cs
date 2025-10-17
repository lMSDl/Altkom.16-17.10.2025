namespace Models
{
    public class AuthData
    {
        //konwencje EF rozpoznają właściwość o nazwie Id lub <nazwa_klasy>Id jako klucz główny
        //jeśli chcemy użyć innej nazwy, musimy to skonfigurować za pomocą Fluent API lub Data Annotations
        public int Key { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}
