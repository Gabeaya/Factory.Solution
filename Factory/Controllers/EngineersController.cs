using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Factory.Models;
using System.Collections.Generic;
using System.Linq;

namespace Factory.Controllers
{
  public class EngineersController : Controller
  {
    private readonly FactoryContext _db;
    public EngineersController(FactoryContext db)
    {
      _db = db;
    }
    
    [HttpGet]
    public ActionResult Index()
    {
      return View(_db.Engineers.ToList());
    }
    
    [HttpGet]
    public ActionResult Details(int id)
    {
        var thisEngineer = _db.Engineers//this produces a list of patient objects from the database
            .Include(engineer => engineer.JoinEntities)//this loades the join entities property of each patient
            .ThenInclude(join => join.Machine)//this loads the doctor of each DoctorPatient relationship
            .FirstOrDefault(engineer => engineer.EngineerId == id);//this specifies which patient from the database were working with
        return View(thisEngineer);
    }
    
    [HttpGet]
    public ActionResult Create()
    {
        ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");
        return View();
    }

    [HttpPost]
    public ActionResult Create(Engineer engineer)
    {
        _db.Engineers.Add(engineer);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public ActionResult Edit(int id)
    {
        var thisEngineer = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
        ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");
        return View(thisEngineer);
    }

    [HttpPost]
    public ActionResult Edit(Engineer engineer)
    {
        _db.Entry(engineer).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public ActionResult AddMachine(int id)
    {
        var thisEngineer = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
        ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Name");
        return View(thisEngineer);
    }

    [HttpPost]
    public ActionResult AddMachine(Engineer engineer, int MachineId)
    {
        if (MachineId != 0)
        {
        _db.EngineerMachine.Add(new EngineerMachine() { MachineId = MachineId, EngineerId = engineer.EngineerId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        var thisEngineer = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
        return View(thisEngineer);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        var thisEngineer = _db.Engineers.FirstOrDefault(engineer => engineer.EngineerId == id);
        _db.Engineers.Remove(thisEngineer);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpPost]
    public ActionResult DeleteMachine(int joinId)
    {
      EngineerMachine joinEntry = _db.EngineerMachine.FirstOrDefault(entry => entry.EngineerMachineId == joinId);
      _db.EngineerMachine.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}