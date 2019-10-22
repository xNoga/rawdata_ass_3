
class Category {
    public int cid { get; set; }
    public string name { get; set; }

    public Category(){}

    public Category (int cid, string name) 
    {
        this.cid = cid;
        this.name = name;
    }
}