public class JsonRequest {
    public string method { get; set; }
    public string path { get; set; }
    public string date { get; set; }
    public object body { get; set; }

    public JsonRequest() {}
    public JsonRequest(string method, string path, string date, object body) {
        this.method = method;
        this.path = path; 
        this.date = date;
        this.body = body;
    }

    public string toString() {
        return "This is request: " + this.method;
    }
}