using ManageMoneyServer.Repositories;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ManageMoneyServer.Validations
{
    public class ContainsInDbAttribute : ValidationAttribute
    {
        private Type RepositoryType { get; set; }
        public ContainsInDbAttribute(Type repositoryType)
        {
            RepositoryType = repositoryType;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value != null)
            {
                var repository = validationContext.GetService(RepositoryType);
                Type serviceType = repository.GetType();
                MethodInfo method = serviceType.GetMethods().FirstOrDefault(m => m.Name == "FindByIdAsync" && m.GetParameters().Length == 1);

                Task task = method.Invoke(repository, new object[] { Convert.ToInt32(value) }) as Task;
                task.Wait();

                PropertyInfo property = task.GetType().GetProperty("Result");
                var result = property.GetValue(task);

                if(result != null)
                {
                    return null;
                }
            }

            return new ValidationResult("");
        }
    }
}
