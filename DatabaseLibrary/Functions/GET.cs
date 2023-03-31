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
                employee = db.Employees.Where(e => e.UserName == userName).Where(e => e.Password == password).First();
            }
            catch { }

            return employee;
        }
    }
}