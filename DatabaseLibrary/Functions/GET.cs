namespace DatabaseLibrary.Functions
{
    public static class GET
    {
        public static Employee? Employee(string userName, string password)
        {
            using ParsethingContext db = new();
            Employee? employee = null;

            try
            {
                employee = db.Employees.Where(e => e.UserName == userName).Where(e => e.Password == password).Include(r => r.Position).First();
            }
            catch { }

            return employee;
        }
        public static int CountOfParsed()
        {
            int count = 0;
            using ParsethingContext db = new();
            try
            {
                count = db.Procurements.Include(r => r.ProcurementState).Where(e => e.ProcurementState.Kind == null).Count();
            }
            catch { }

            return count;
        }

    }
}