class JsonResponse {
    public string status { get; set; }
    public string body { get; set; }

    public JsonResponse() {}

    public JsonResponse(string status) {
        this.status = status;
    }
    public JsonResponse(string status, string body) {
        this.status = status;
        this.body = body; 
    }
}