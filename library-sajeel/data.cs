    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Security.Cryptography;
    using System.IO;

    namespace project_data
    {
        class data
        {
            private string usersFileName = "users.txt";
            private string adminsFileName = "admins.txt";
            private string bookFileName = "books.txt";
            private string loanFileName = "loanedBooks.txt";
            private string reserveFilename = "reservedBooks.txt";
            private string byteToString(byte[] byteHash)
            {
                int i;
                StringBuilder sOutput = new StringBuilder(byteHash.Length);
                for (i = 0; i < byteHash.Length; i++)
                {
                    sOutput.Append(byteHash[i].ToString("X2"));

                }
                return sOutput.ToString();
            }

      
            public string hashPassword(string password)
            {
                byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(password);
                byte[] tmpHash = new SHA256CryptoServiceProvider().ComputeHash(tmpSource);

                return byteToString(tmpHash);
            }

            public string[] getUserIterator()
            {
                string[] lines = File.ReadAllLines(usersFileName);
                return lines;
            }

            public string[] getAdminIterator()
            {
                string[] lines = File.ReadAllLines(adminsFileName);
                return lines;
            }

            public void addMember(string username, string password, string firstname, string lastname, bool admin)
            {
                string filename = usersFileName;
                if (admin)
                {
                    filename = adminsFileName;
                }
                string[] lines =
                {
                    $"{username} {hashPassword(password)} {firstname.Replace(' ', '-')} {lastname.Replace(' ', '-')}"
                };
                File.AppendAllLines(filename, lines);
            }

            public void writeToFile(string[] text, bool admin = true, bool append = true)
            {
                string filename = usersFileName;
                if (admin)
                {
                    filename = adminsFileName;
                }
                if (append)
                {
                    File.AppendAllLines(filename, text);
                }
                else
                {
                    File.WriteAllLines(filename, text);
                }
            }
            public bool userInDataBase(string personnummer, string password, bool user)
            {
                string[] iterator;
                if (user)
                {
                    iterator = getUserIterator();
                }
                else
                {
                    iterator = getAdminIterator();
                }
                foreach (var line in iterator)
                {
                    if (password == " ")
                    {
                        if (line.Split(' ')[0] == personnummer)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (line.Split(' ')[0] == personnummer && this.hashPassword(password) == line.Split(' ')[1])
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public string[] getBookIterator()
            {
                string[] lines = File.ReadAllLines(bookFileName);
                return lines;
            }

            public string[] getLoanIterator()
            {
                string[] lines = File.ReadAllLines(loanFileName);
                return lines;
            }

            public string[] getReserveIterator()
            {
                string[] lines = File.ReadAllLines(reserveFilename);
                return lines;
            }

            public void writeToBookFile(string[] lines, bool append = true)
            {
                if (append)
                {
                    File.AppendAllLines(bookFileName, lines);
                }
                else
                {
                    File.WriteAllLines(bookFileName, lines);
                }
            }
            public void writeToLoanFile(string[] lines, bool append = true)
            {
                if (append)
                {
                    File.AppendAllLines(loanFileName, lines);
                }
                else
                {
                    File.WriteAllLines(loanFileName, lines);
                }
            }

            public void writeToReserveFile(string[] lines, bool append = true)
            {
                if (append)
                {
                    File.AppendAllLines(reserveFilename, lines);
                }
                else
                {
                    File.WriteAllLines(reserveFilename, lines);
                }
            }
        }
    }