using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace vastwap
{
    public partial class index : System.Web.UI.Page
    {
        static string ConnString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        static SqlConnection con = new SqlConnection(ConnString);
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            User user = new User();
            user.email = txtEmail.Text;
            user.password = Hash.getHashSha256(txtPassword.Text);

            if (user.ValidateUser()) //Record exists
            {
                con.Open();
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "SELECT u.UserID, u.TypeID, t.UserType FROM Users u " +
                    "INNER JOIN Types t on u.TypeID = t.TypeID " +
                    "WHERE Email=@Email AND Password=@Password";
                com.Parameters.AddWithValue("@Email", user.email);
                com.Parameters.AddWithValue("@Password", user.password);
                SqlDataReader dr = com.ExecuteReader();
                if (dr.HasRows) // email and password match
                {
                    while (dr.Read())
                    {
                        Session["userid"] = dr["UserID"].ToString();
                        string usertype = dr["UserType"].ToString();
                        switch (usertype)
                        {
                            case "Regular": con.Close(); 
                                Response.Redirect("~/pages/client/index_si.aspx");
                                break;
                            case "SuperAdmin": con.Close(); 
                                Response.Redirect("~/pages/main/index_vip.aspx");
                                break;
                            case "VIP": con.Close();
                                Response.Redirect("~/pages/client/index_vip.aspx");
                                break;
                            case "Cashier": con.Close();
                                Response.Redirect("~/pages/cashier/index.aspx");
                                break;
                            case "Driver": con.Close();
                                Response.Redirect("~/pages/driver/index.aspx");
                                break;
                        }
                    }                    
                }
                else // email and password don't match
                {
                    con.Close();
                }
            }
            else
            {
                Response.Redirect("~/pages/main/index.aspx");
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            User user = new User();
            Clientc client = new Clientc();
            Car car = new Car();

            user.email = txtEmailReg.Text;
            if(user.ValidateEmail()) // First Checking : Is Email Valid?
            {
                if (txtPasswordReg.Text == txtConfirmPassword.Text) // Second Checking : Do passwords match?
                {
                    // Assign USER and CLIENT values.

                    user.password = Hash.getHashSha256(txtPasswordReg.Text);

                    client.firstName = txtFirstName.Text;
                    client.lastName = txtLastName.Text;
                    client.mobile = txtMobile.Text;
                    client.birthdate = DateTime.Parse(txtBirthday.Text);

                    car.id = txtPlate.Text;
                    if (car.ValidatePlate()) // Third Checking : Is Plate already in use?
                    {
                        // Assign CAR values.
                        car.model = txtCarModel.Text;
                        car.color = txtCarColor.Text;
                    }
                    else // Third Checking.
                    {
                        txtPlate.Focus();
                    }
                }
                else // Second Checking.
                {
                    txtPasswordReg.Focus();
                }
            }
            else // First Checking.
            {
                txtEmailReg.Focus();
            }
        }
    }
}