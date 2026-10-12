using AcxiomCRM.Data; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AcxiomCRM.Controllers;
[Authorize] public class DashboardController(ApplicationDbContext db) : Controller {
 public async Task<IActionResult> Index(){var isAdmin=User.IsInRole("Admin");var isManager=User.IsInRole("Manager");var uid=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
 var customers=db.Customers.AsQueryable();var leads=db.Leads.AsQueryable();var opps=db.Opportunities.AsQueryable();var followups=db.FollowUps.AsQueryable(); if(!isAdmin&&!isManager){customers=customers.Where(x=>x.OwnerId==uid);leads=leads.Where(x=>x.AssignedTo==uid);opps=opps.Where(x=>x.AssignedTo==uid);followups=followups.Where(x=>x.AssignedTo==uid);}
 ViewBag.Customers=await customers.CountAsync();ViewBag.Leads=await leads.CountAsync();ViewBag.OpenLeads=await leads.CountAsync(x=>x.Status!="Lost"&&x.Status!="Converted");ViewBag.Opportunities=await opps.CountAsync();ViewBag.OpenOpportunities=await opps.CountAsync(x=>x.Status=="Open");ViewBag.Won=await opps.CountAsync(x=>x.Stage=="Won");ViewBag.Lost=await opps.CountAsync(x=>x.Stage=="Lost");ViewBag.Pipeline=await opps.Where(x=>x.Status=="Open").SumAsync(x=>(decimal?)x.Amount)??0m;
 ViewBag.LeadLabels=new[]{"New","Contacted","Qualified","Lost","Converted"};ViewBag.LeadValues=await Task.WhenAll(new[]{"New","Contacted","Qualified","Lost","Converted"}.Select(async s=>await leads.CountAsync(x=>x.Status==s)));
 ViewBag.StageLabels=new[]{"Qualification","Proposal","Negotiation","Won","Lost"};ViewBag.StageValues=await Task.WhenAll(new[]{"Qualification","Proposal","Negotiation","Won","Lost"}.Select(async s=>await opps.CountAsync(x=>x.Stage==s))); return View(); }
}
