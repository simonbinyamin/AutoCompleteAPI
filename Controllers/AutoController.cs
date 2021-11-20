using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using AutoCompleteAPI.Models;
using AutoCompleteAPI.Environment;

namespace AutoCompleteAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AutoController : ControllerBase
{
    private HttpClient client;
    private Secret secret;
    public AutoController()
    {
        client = new HttpClient();
        secret = new Secret();
    }
    
    [HttpGet("Create/{index}/{id}/{key}")]
    public async Task<string> Create(string index,string id, string key, string? name = null)
    {      
        if(key != secret.KEY) {
            return "Not authorized";
        } else {
            name = Request.Query["name"];

            using (var _client = new HttpClient())
            {

                JObject jsonObject = new JObject
                {
                    [index] = new JObject
                    {
                        ["Id"] = id,
                        ["Name"] = name
                    }
                };

                HttpContent content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");

                var result = await _client.PatchAsync(secret.AUTO_LST+".json?auth=" + secret.API_KEY, content);

            return await result.Content.ReadAsStringAsync();
            }
        }
    }
    
    [HttpDelete("Delete/{index}/{key}")]
    public async Task<string> Delete(string index, string key)
    {
        if(key != secret.KEY) {
            return "Not authorized";
        } else {
            using (var _client = new HttpClient())
            {
                var result = await _client.DeleteAsync(secret.AUTO_LST +"/"+ index + ".json?auth=" + secret.API_KEY);
                
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
    
    [HttpGet]
    [Route("List")]
    public async Task<Item[]?> List()
    {
        string resultContent = "";

        HttpResponseMessage response = await client.GetAsync(secret.AUTO + ".json");
        resultContent = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(resultContent);
        var feat = json?["lst"] ?? "";
        return feat.ToObject<Item[]>();
    }
}