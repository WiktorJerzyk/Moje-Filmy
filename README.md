# PROJEKT MOJE FILMY
Projekt ten składa się z następujących komponentów:
* **BACKEND/API**: Stworzone przy użyciu .NET
* **FRONTEND/UI**: Stworzone przy użyciue Vue JS
* **STYLE**: Zaimplementowane przy pomocy BOOTSTRAP 5
* **Baza Danych**: API wysyła REQUESTY do bazy danych utworzonej za pomocą *Neon Cloud PostgreSQL*. Baza danych w projekcie jest jawna ze względu na ułatwienie dostępu i ułatwienie konfiguracji projektu - jest to rozwiązanie niebezpieczne, ale prostsze w implementecji.

# Wymagania
- .NET SDK 6.0 (lub nowszy) dla API
- VS 2022 z odpowiednimi rozszerzeniami

# PORTY z których domyślnie korzysta projekt:
- API korzysta z portu 5000, na pętli zwrotnej
- UI korzysta z portu 3500, na pętli zwrotnej

# Instrukcja Uruchomienia
- Klonowanie Repozytorium do folderu: np. **gh repo clone WiktorJerzyk/Moje-Filmy**
- Następnie trzeba przejść do folderu w którym jest zawartość repozytorium **cd Moje-Filmy**
- Odpalenie API: **cd /api/Project**
- * dotnet restore
  * dotnet run
- Następnie otworzyć nowy terminal (np. vs code) i wpisać **cd ui/biblioteka-ui**
- * npm install
  * npm run dev
