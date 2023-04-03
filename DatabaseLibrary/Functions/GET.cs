namespace DatabaseLibrary.Functions;

public static class GET
{
    public static Employee? Employee(string userName, string password)
    {
        using ParsethingContext db = new();
        Employee? employee = null;

        try
        {
            employee = db.Employees.Include(e => e.Position).Where(e => e.UserName == userName).Where(e => e.Password == password).First();
        }
        catch { }

        return employee;
    }

    public static int CountOfParsed()
    {
        using ParsethingContext db = new();
        int count = 0;

        try
        {
            count = db.Procurements.Where(p => p.ProcurementStateId == null).Count();
        }
        catch { }

        return count;
    }
}