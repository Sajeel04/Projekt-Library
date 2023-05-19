using System;
using System.Collections.Generic;
using System.Text;
using project_data;
using project_library;

namespace project_user
{
    class User
    {
        public data d = new data();
        public string personnummer;
        public string password;
        public string firstname;
        public string lastname;
        public bool success = false;
    }

    class Member : User
    {
        public Member(string personnumer, string password, string firstname, string lastname, bool newAccount)
        {
            this.personnummer = personnumer;
            this.password = password;
            this.firstname = firstname;
            this.lastname = lastname;
            this.success = true;
            if (newAccount)
            {
                if (d.userInDataBase(personnummer, " ", true))
                {
                    Console.WriteLine($"Användare med personnummer {this.personnummer} redan registrerad");
                    this.success = false;
                }
                if (this.success)
                {
                    Console.WriteLine($"Lägger till användare med personnummer {this.personnummer}");
                    d.addMember(this.personnummer, this.password, firstname, lastname, false);
                    string[] lines =
                    {
                        $"{this.personnummer}"
                    };

                    d.writeToLoanFile(lines, true);
                }
            }

            else if (this.d.userInDataBase(this.personnummer, this.password, true))
            {
                Console.WriteLine($"Du är inloggad med personnumret {this.personnummer}");
                this.success = true;
            }

            else if (!this.d.userInDataBase(this.personnummer, " ", true))
            {
                Console.WriteLine($"Hittade inget registrerat konto med personnummer {this.personnummer}");
                this.success = false;
            }
            else
            {
                Console.WriteLine($"Fel lösenord {this.personnummer}");
                this.success = false;
            }
        }
        public void changePassword(string newPassword)
        {
            string[] iterator = d.getUserIterator();
            for (int i = 0; i < iterator.Length; i++)
            {
                string[] credentials = iterator[i].Split(' ');
                if (this.personnummer == credentials[0])
                {
                    iterator[i] = $"{credentials[0]} {d.hashPassword(newPassword)} {credentials[2]} {credentials[3]}";
                    d.writeToFile(iterator, false, false);
                    break;
                }
            }
        }
    }

    class Librarian : User
    {
        private string code = "1234";
        public Librarian(string librarianpersonnummer, string password, string firstname, string lastname, bool newAccount)
        {
            this.personnummer = librarianpersonnummer;
            this.password = password;
            this.firstname = firstname;
            this.lastname = lastname;
            if (newAccount)
            {
                Console.Write("Skriv in säkerhetskod för att skapa nytt admin-konto: ");
                string inputCode = Console.ReadLine();
                if (inputCode == code)
                {
                    if (!d.userInDataBase(this.personnummer, " ", false))
                    {
                        d.addMember(this.personnummer, this.password, firstname, lastname, true);
                        Console.WriteLine($"Skapade nytt admin-konto med personnummer {this.personnummer}");
                        this.success = true;
                    }
                    else
                    {
                        Console.WriteLine($"Admin-konto med personnummer {this.personnummer} finns redan");
                        this.success = false;
                    }
                }
                else
                {
                    Console.WriteLine("Fel säkerhetskod");
                    this.success = false;
                }
            }
            else if (!newAccount)
            {
                this.personnummer = librarianpersonnummer;
                this.password = password;
                this.success = true;
                if (this.d.userInDataBase(this.personnummer, this.password, false))
                {
                    Console.WriteLine($"Du är inloggad som admin med personnumret {this.personnummer}");
                    this.success = true;
                }
                else if (this.d.userInDataBase(this.personnummer, " ", false))
                {
                    Console.WriteLine($"Fel lösenord");
                    this.success = false;
                }
                else
                {
                    Console.WriteLine($"Kunde inte hitta admin-konto med personnummer {this.personnummer}");
                    this.success = false;
                }
            }

            else
            {
                Console.WriteLine("Fel säkerhetskod.");
            }
        }
        public void removeUser(string personnummer)
        {
            bool removed = false;
            string[] users = d.getUserIterator();
            string[] loanBooks = d.getLoanIterator();
            string[] reserveBooks = d.getReserveIterator();
            string[][] userFiles = { users, loanBooks };
            for (int i = 0; i < userFiles.Length; i++)
            {
                for (int j = 0; j < userFiles[i].Length; j++)
                {
                    if (userFiles[i][j].Split(' ')[0] == personnummer)
                    {
                        userFiles[i][j] = "";
                        var foos = new List<string>(userFiles[i]);
                        foos.RemoveAt(j);
                        if (i == 0)
                        {
                            d.writeToFile(foos.ToArray(), false, false);
                        }
                        if (i == 1)
                        {
                            d.writeToLoanFile(foos.ToArray(), false);
                            removed = true;
                        }

                        break;
                    }
                }

            }

            if (removed)
            {
                for (int i = 0; i < reserveBooks.Length; i++)
                {
                    string[] reservedUsers = reserveBooks[i].Split(' ');
                    reserveBooks[i] = reservedUsers[0];
                    for (int j = 1; j < reservedUsers.Length; j++)
                    {
                        if (reservedUsers[j] != personnummer)
                        {
                            reserveBooks[i] += $" {reservedUsers[j]}";
                        }
                    }

                }

                d.writeToReserveFile(reserveBooks, false);
                Console.WriteLine($"Tog bort användare med personnummer {personnummer}");

            }

            else
            {
                Console.WriteLine($"Hittade inte användare med personnummer {personnummer}");
            }
        }

        public List<string> listUsers()
        {
            string[] users = this.d.getUserIterator();
            List<string> usersPersonnummer = new List<string>();
            for (int i = 0; i < users.Length; i++)
            {
                string[] line = users[i].Split(' ');
                usersPersonnummer.Add(line[0]);
                Console.WriteLine($"{i + 1} - {line[0]} { line[2].Replace('-', ' ')} { line[3].Replace('-', ' ') }");
            }

            return usersPersonnummer;
        }

        public void editMember(string personnummer, string[] newCredentials)
        {
            string[] users = d.getUserIterator();
            bool edited = false;
            bool passwordChange = true;
            bool personnummerChange = true;
            for (int i = 0; i < users.Length; i++)
            {
                string[] credentials = users[i].Split(' ');
                if (credentials[0] == personnummer)
                {
                    for (int j = 0; j < newCredentials.Length; j++)
                    {
                        if (newCredentials[j] == " ")
                        {
                            if (j == 0)
                            {
                                personnummerChange = false;
                            }
                            if (j == 1)
                            {
                                passwordChange = false;
                            }

                            newCredentials[j] = credentials[j];
                        }
                    }
                    if (passwordChange)
                    {
                        newCredentials[1] = d.hashPassword(newCredentials[1]);
                    }
                    users[i] = $"{newCredentials[0]} {newCredentials[1]} {newCredentials[2].Replace(' ', '-')} {newCredentials[3].Replace(' ', '-')}";
                    d.writeToFile(users, false, false);

                    edited = true;
                    break;
                }
            }

            if (!edited)
            {
                Console.WriteLine($"Kunde inte hitta användare med personnummer {personnummer}");
            }

            else
            {
                Console.WriteLine($"Ändrade uppgifter för användare med personnummer {personnummer}");

                if (personnummerChange)
                {
                    string[] loanFile = d.getLoanIterator();
                    for (int i = 0; i < loanFile.Length; i++)
                    {
                        string[] line = loanFile[i].Split(' ');
                        if (line[0] == personnummer)
                        {
                            loanFile[i] = newCredentials[0];
                            for (int j = 1; j < line.Length; j++)
                            {
                                loanFile[i] += $" {line[j]}";
                            }
                            d.writeToLoanFile(loanFile, false);
                            break;
                        }
                    }

                    string[] reserveFile = d.getReserveIterator();
                    for (int i = 0; i < reserveFile.Length; i++)
                    {
                        string[] line = reserveFile[i].Split(' ');
                        reserveFile[i] = line[0];
                        for (int j = 1; j < line.Length; j++)
                        {
                            if (line[j] == personnummer)
                            {
                                reserveFile[i] += $" {newCredentials[0]}";
                            }
                            else
                            {
                                reserveFile[i] += $" {line[j]}";
                            }
                        }
                    }
                    d.writeToReserveFile(reserveFile, false);
                }
            }
        }
    }
}