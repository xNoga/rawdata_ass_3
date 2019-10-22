using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

class CategoryDB
{

    const string TCP_OK = "1 Ok";
    const string TCP_CREATED = "2 Created";
    const string TCP_UPDATED = "3 Updated";
    const string TCP_BADREQUEST = "4 Bad Request";
    const string TCP_NOTFOUND = "5 Not Found";
    const string TCP_ERROR = "6 Error";
    string[] validMethods = { "create", "read", "update", "delete", "echo" };
    JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };

    public string delegater(JsonRequest request)
    {
        // if echo just return here
        if (request.method == "echo")
        {
            string res = "";
            if (request.body == null) return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_OK + " " + "missing body"), options);
            else return JsonSerializer.Serialize<echoRes>(new echoRes(TCP_OK, request.body.ToString()), options);
        }

        // validating request - abort if not valid
        var response = validateRequest(request);
        if (response != "") return response;

        // validate the given path in request (because of stupid tests)
        var pathRes = validatePath(request);
        if (pathRes != "") return pathRes;

        Regex re = new Regex(@"^\/api\/categories\/\d+$");
        Match id = re.Match(request.path);

        // getting an id (if any exists)
        int cid = -1;
        if (id.Success)
        {
            string pattern = @"\D+";
            string[] regexRes = Regex.Split(request.path, pattern);
            try
            {
                cid = int.Parse(regexRes[1]);
            }
            catch (FormatException e) { System.Console.WriteLine(e.Message); }
        }


        switch (request.method)
        {
            case "read":
                if (cid != -1)
                {
                    return read(cid);
                }
                else
                {
                    return read();
                }
            case "create":
                if (cid != -1) return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST), options);
                var category = JsonSerializer.Deserialize<Category>(request.body.ToString());
                if (category.name == null || category.name == "") return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST), options);
                return create(category);
            case "update":
                if (cid == -1) return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST), options);
                var catToUpdate = JsonSerializer.Deserialize<Category>(request.body.ToString());
                if (catToUpdate.name == null || catToUpdate.name == "") return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST), options);
                return update(catToUpdate, cid);
            case "delete":
                if (cid == -1) return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST), options);
                return delete(cid);
            default:
                System.Console.WriteLine("Not a valid method");
                return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST), options);
        }
    }

    public string validateRequest(JsonRequest request)
    {
        string errorStr = "";
        // validate method
        if (request.method == null) errorStr += "missing method, ";
        if (request.method != null && (Array.IndexOf(validMethods, request.method) == -1)) errorStr += "illegal method, ";
        // validate path
        if (request.path == null) errorStr += "missing resource, ";

        // validate date
        if (request.date == null) errorStr += "missing date, ";
        if (request.date != null)
        {
            try
            {
                var longDate = Convert.ToInt64(request.date);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(longDate);
            }
            catch (FormatException e)
            {
                System.Console.WriteLine(e.Message);
                errorStr += "illegal date, ";
            }
        }

        // validate body
        if (request.method == "create" || request.method == "update")
        {
            if (request.body == null || request.body.ToString() == "")
            {
                errorStr += "missing body, ";
            }
            else
            {
                try
                {
                    var cat = JsonSerializer.Deserialize<Category>(request.body.ToString());
                }
                catch (Exception ex) { errorStr += "illegal body"; Console.WriteLine(ex.Message); }
            }
        }

        if (errorStr != "")
        {
            return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST + " " + errorStr), options);
        }

        return "";
    }

    public string validatePath(JsonRequest request)
    {
        if (request.path != null)
        {
            Regex re = new Regex(@"^(\/api\/categories)(\/\d*)?$");
            Match id = re.Match(request.path);
            string missing = "";
            if (!id.Success) return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_BADREQUEST + missing), options);
        }
        return "";
    }

    public string read()
    {
        Category[] cats = assignment_3.Program.categories.ToArray();
        var catAsJson = JsonSerializer.Serialize<Category[]>(cats, options);
        var response = JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_OK, catAsJson), options);
        return response;
    }

    public string read(int cid)
    {
        var res = new JsonResponse(TCP_NOTFOUND);

        foreach (var cat in assignment_3.Program.categories)
        {
            if (cat.cid == cid)
            {
                res.status = TCP_OK;
                var catAsJson = JsonSerializer.Serialize<Category>(cat, options);
                res.body = catAsJson;
            }
        }

        return JsonSerializer.Serialize<JsonResponse>(res, options);
    }

    public string create(Category cat)
    {
        cat.cid = assignment_3.Program.categories.Count + 1;
        assignment_3.Program.categories.Add(cat);
        var catAsJson = JsonSerializer.Serialize<Category>(cat, options);
        return JsonSerializer.Serialize<JsonResponse>(new JsonResponse(TCP_CREATED, catAsJson), options);
    }

    public string update(Category cat, int cid)
    {
        var res = new JsonResponse(TCP_NOTFOUND);
        var catAsJson = "";
        foreach (var c in assignment_3.Program.categories)
        {
            if (c.cid == cid)
            {
                c.name = cat.name;
                cat.cid = cid;
                res.status = TCP_UPDATED;
                catAsJson = JsonSerializer.Serialize<Category>(cat, options);
            }
        }
        return JsonSerializer.Serialize<JsonResponse>(res, options);
    }

    public string delete(int cid)
    {
        var res = new JsonResponse(TCP_NOTFOUND);

        foreach (var cat in assignment_3.Program.categories.ToArray())
        {
            if (cat.cid == cid)
            {
                assignment_3.Program.categories.Remove(cat);
                res.status = TCP_OK;
            }
        }

        return JsonSerializer.Serialize<JsonResponse>(res, options);
    }
}

class echoRes
{
    public string status { get; set; }
    public string body { get; set; }

    public echoRes(string status, string body)
    {
        this.status = status;
        this.body = body;
    }
}