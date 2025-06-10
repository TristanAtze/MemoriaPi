using MemoriaPiDataCore.Models;
using MemoriaPiDataCore.SQL;
namespace MemoriaPiDataCore.LocalStorage;

public class LocalSetup
{
    public static void SetupDataStructure()
    {
        //List<ApplicationUser> currentUsers = SqlReader.GetCachedUsers();
        
        //if (!Directory.Exists("Users"))
        //    Directory.CreateDirectory("Users");

        
        //foreach (ApplicationUser currentUser in currentUsers)
        //{
        //    if (!Directory.Exists("Users\\" + Convert.ToString(currentUser.Id)))
        //        Directory.CreateDirectory("Users\\" + Convert.ToString(currentUser.Id));

            
        //    if (!Directory.Exists("Users\\" + Convert.ToString(currentUser.Id) + "Media"))
        //        Directory.CreateDirectory("Users\\" + Convert.ToString(currentUser.Id) + "\\Media");
            
        //    if (!Directory.Exists("Users\\" + Convert.ToString(currentUser.Id) + "\\Files"))
        //        Directory.CreateDirectory("Users\\" + Convert.ToString(currentUser.Id) + "\\Files");
            
        //}
    }
}