using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Itmo.MyWorkWithDB.RottenTomatoes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    



    public partial class MainWindow : Window
    {
	    NpgsqlConnection cnStreaming = new NpgsqlConnection(cnStreamingString);
		public MainWindow()
        {
            InitializeComponent();
            LoadMainPage();

        }
        
        private void LoadMainPage()
        {
            movies.Clear();
			lstMainPage.Items.Clear();
			lstIMBD.Items.Clear();
			lstRottenTomatoes.Items.Clear();
			string requestSSql = $@"select title, imdbrating , movieid , rottentomatoes
                                             from movies
                                             order by movieid 
                                             limit {sizePage} offset {offset}";
			try
            {
                cnStreaming.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(requestSSql,cnStreaming);
                NpgsqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					string item = reader.GetString(0) ;
					lstMainPage.Items.Add(item);
					
					
					if (!reader.IsDBNull(reader.GetOrdinal("imdbrating")))
					{
						lstIMBD.Items.Add(reader.GetDouble(1));
					}
								

						if (!reader.IsDBNull(reader.GetOrdinal("rottentomatoes")))
					{
						lstRottenTomatoes.Items.Add(reader.GetDouble(3));

					}
					


						if (!movies.ContainsKey(item))
						{
							movies.Add(item, reader.GetInt32(2));
						}
					}
				


            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if(cnStreaming.State != System.Data.ConnectionState.Closed)
                {
                    cnStreaming.Close();
                }
                

            }
        }

		private void LoadMainPage(string requestSql)
		{
			movies.Clear();
			lstMainPage.Items.Clear();
			lstIMBD.Items.Clear();
			lstRottenTomatoes.Items.Clear();
			
			try
			{
				cnStreaming.Open();
				NpgsqlCommand cmd = new NpgsqlCommand(requestSql, cnStreaming);
				NpgsqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					string item = reader.GetString(0);
					lstMainPage.Items.Add(item);


					if (!reader.IsDBNull(reader.GetOrdinal("imdbrating")))
					{
						lstIMBD.Items.Add(reader.GetDouble(1));
					}
					
					if (!reader.IsDBNull(reader.GetOrdinal("rottentomatoes")))
					{
						lstRottenTomatoes.Items.Add(reader.GetDouble(3));

					}
					
					if (!movies.ContainsKey(item))
					{
						movies.Add(item, reader.GetInt32(2));
					}
				}



			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (cnStreaming.State != System.Data.ConnectionState.Closed)
				{
					cnStreaming.Close();
				}


			}
		}
		private void LoadAdditinalPage(ListBox lstElement, string requestSql)
		{
			
			try
			{
				cnStreaming.Open();
				NpgsqlCommand cmd = new NpgsqlCommand(requestSql, cnStreaming);
				NpgsqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					string item = reader.GetString(0) ;

					lstElement.Items.Add(item);

					
				}


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (cnStreaming.State != System.Data.ConnectionState.Closed)
				{
					cnStreaming.Close();
				}


			}
		}

		private void LoadDescriptionPage(string requestSql)
		{
			
			try
			{
				cnStreaming.Open();
				NpgsqlCommand cmd = new NpgsqlCommand(requestSql, cnStreaming);
				NpgsqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
                   if (reader.IsDBNull(0))
                    
                    {
                        txtDescriptionBox.Text = "There is no description yet";
                    }
                    else
                    {
                        txtDescriptionBox.Text = (string)reader.GetString(0); 
                    }
					

					
				}


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (cnStreaming.State != System.Data.ConnectionState.Closed)
				{
					cnStreaming.Close();
				}


			}
		}

		
		
		

        private void lstMainPage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            lstCastPage.Items.Clear();
			lstReview.Items.Clear();
			btnTypeReview.IsEnabled = true;
            int movieid = movies[lstMainPage.SelectedItem.ToString()];
                   
            string requestCastSql = $"select castname from moviescast where movieid = {movieid}";
            string requestDiscriptionSql = $"select description from movies where movieid= {movieid};";
			string requestReviewSql = $"select reviewtext from reviews where movieid = {movieid}";
			LoadAdditinalPage(lstCastPage, requestCastSql);
			LoadAdditinalPage(lstReview, requestReviewSql);
            LoadDescriptionPage(requestDiscriptionSql);

		}

		private void btnReview_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(txtDescriptionBox.Text))
			{
				txtDescriptionBox.Focus();
			}
			else
			{
				try
				{
					btnReview.Visibility = Visibility.Hidden;
					string addreviews = txtDescriptionBox.Text;
					int movieid = movies[lstMainPage.SelectedItem.ToString()];
					string updateReviewsSql = $"insert into reviews (movieid, reviewtext) values({movieid},'{addreviews}');";


					cnStreaming.Open();
					NpgsqlCommand cmd = new NpgsqlCommand(updateReviewsSql, cnStreaming);


					int updateRowNumber = cmd.ExecuteNonQuery();
					MessageBox.Show($"Thanks for review");




				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					if (cnStreaming.State != System.Data.ConnectionState.Closed)
					{
						cnStreaming.Close();
					}
					txtDescriptionBox.Clear();


				}
			}
		


	}

		private void btnTypeReview_Click(object sender, RoutedEventArgs e)
		{

			txtDescriptionBox.Clear();
			txtDescriptionBox.Focus();
			btnReview.Visibility = Visibility.Visible;
			

		}

		

		private void btnSortUpIMBD_Click(object sender, RoutedEventArgs e)
		{
		   
		   LoadMainPage(sortUpIMBDSql);

		}

		private void btnSortDownIMBD_Click(object sender, RoutedEventArgs e)
		{
			LoadMainPage(sortDownIMBDSql);
		}

		private void btnSortDownTomatoes_Click(object sender, RoutedEventArgs e)
		{
			LoadMainPage(sortDownTomatoesSql);

		}

		private void btnSortUpTomatoes_Click(object sender, RoutedEventArgs e)
		{
			LoadMainPage(sortUpTomatoesSql);

		}

		private void btnNextPage_Click(object sender, RoutedEventArgs e)
		{
			offset += sizePage;
			LoadMainPage();
		}

		private void btnPrevpage_Click(object sender, RoutedEventArgs e)
		{
			offset = Math.Max(0, offset - sizePage);
			LoadMainPage();


		}
		// Retrieve a connection string by specifying the providerName.
		// Assumes one connection string per provider in the config file.

		static string GetConnectionStringByProvider(string providerName)
		{
			// Get the collection of connection strings.
			ConnectionStringSettingsCollection settings =
				ConfigurationManager.ConnectionStrings;

			// Walk through the collection and return the first
			// connection string matching the providerName.
			if (settings != null)
			{
				foreach (ConnectionStringSettings cs in settings)
				{
					if (cs.ProviderName == providerName)
					{
						return cs.ConnectionString;
					}
				}
			}
			return null;
		}
		// Retrieves a connection string by name.
		// Returns null if the name is not found.
		static string GetConnectionStringByName(string name)
		{
			// Look for the name in the connectionStrings section.
			ConnectionStringSettings settings =
				ConfigurationManager.ConnectionStrings[name];

			// If found, return the connection string (otherwise return null)
			return settings.ConnectionString;
		}

		

		private static string cnName = "Streaming";
		private static string cnStreamingString = GetConnectionStringByName(cnName);

		private static string sortUpIMBDSql = $@"select title, imdbrating , movieid , rottentomatoes
                                             from movies
                                               order by imdbrating asc limit {sizePage} ";
		private static string sortDownIMBDSql = $@"select title, imdbrating , movieid , rottentomatoes
                                             from movies
                                               order by imdbrating desc limit {sizePage}";
		private static string sortUpTomatoesSql = $@"select title, imdbrating , movieid , rottentomatoes
                                             from movies
                                               order by rottentomatoes limit {sizePage}";
		private static string sortDownTomatoesSql = $@"select title, imdbrating , movieid , rottentomatoes
                                             from movies
                                               order by rottentomatoes desc limit {sizePage}";
		private const int sizePage = 20;
		
		private static int offset = 0;


		private Dictionary<string, int> movies = new Dictionary<string, int>();
	}

}
