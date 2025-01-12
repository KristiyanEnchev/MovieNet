# MovieNet 🎥 

## 🏗️ Overviw

**MovieNet** is a movie discovery and management app, built with **.NET 6** and **React 18 (Vite)**, leveraging **Clean Architecture**, **DDD**, **TMDB API**, **Redis**, **PostgreSQL**, and modern frontend technologies like TailwindCSS, shadcn/ui, and **RTK Query** for a dynamic and scalable user experience. It’s containerized with **Docker** and serves the UI through **Nginx**.

---

## 👀 Try it out

To run the project locally:

```bash
git clone https://github.com/KristiyanEnchev/MovieNet.git
cd MovieNet/
cp .env.example .env
docker-compose up --build -d
```

---

## 🔠 Configuration

- **Default Account**: Email: admin@admin.com, Password: 123456
- **TMDB API Key**: Set your TMDB API key at `https://www.themoviedb.org` and add it in the `.env` file.
- **Database**: PostgreSQL. DB UI - `http://localhost:5050` - Acc: **admin** - Pass: **password**
- **Cache**: Redis is used for caching. Redis UI - `http://localhost:8081` - Acc: **masterauth** - Pass: **password** 
- **API URL**: `http://localhost:5069`
- **UI URL**: `http://localhost:3000`

---

## 🔧 Built with

### Backend:
- [.NET 6](https://dotnet.microsoft.com/)
- [ASP.NET Core 6](https://github.com/dotnet/aspnetcore)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [AutoMapper](https://github.com/AutoMapper/AutoMapper)
- [CQRS & MediatR](https://github.com/jbogard/MediatR)
- [Result Pattern](https://enterprisecraftsmanship.com/posts/functional-c-handling-failures-input-errors/) for robust error handling.
- [Serilog](https://github.com/serilog/serilog) for structured logging.
- [Redis](https://redis.io/) for caching.
- [PostgreSQL](https://www.postgresql.org/) as the relational database.
- **Custom Background Jobs**: For asynchronous tasks and scheduled operations.
- **Unit Testing**: xUnit, Moq, Shouldly.

### Frontend:
- [React 18](https://reactjs.org/) 
- [Vite](https://vitejs.dev/) 
- [TailwindCSS](https://tailwindcss.com/) 
- [FramerMotion](https://github.com/framer/motion) 
- [Redux Toolkit](https://github.com/reduxjs/redux-toolkit) 
- [Redux Toolkit Query](https://github.com/reduxjs/redux-toolkit) 
- [React Router Dom](https://reactrouter.com/) 
- [FontAwesome](https://fontawesome.com/) 
- [Zod](https://github.com/colinhacks/zod) 

### DevOps:
- **Docker & Docker Compose**: Simplified containerization and orchestration.
- **Nginx**: High-performance reverse proxy for serving the frontend.

---

## 📚 Features
- **Movie Search**: Search and discover movies.
- **Movie Sort**: Sort your movies per genre.
- **Trailer Preview**: Watch your favorite movies trailer.
- **Watchlist**: Save your favorite movies to your watchlist.
- **Like system**: Like your favorite  movies.
- **Responsive Design**: A seamless experience on all devices.
- **High Performance**: Leveraging Redis caching and Serilog logging.
- **Custom Background Jobs**: Efficient processing of asynchronous and scheduled tasks.

---

## ✏️ License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Show your support

Give a ⭐ if you like this project and find it helpful!

---

## 🧏‍♂️️ Author 

[![Facebook](https://img.shields.io/badge/kristiyan.enchev-%231877F2.svg?style=for-the-badge&logo=Facebook&logoColor=white)](https://www.facebook.com/kristiqn.enchev.5/) [![Instagram](https://img.shields.io/badge/kristiyan-%23E4405F.svg?style=for-the-badge&logo=Instagram&logoColor=white)](https://www.instagram.com/kristiyan_e/)[
![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:kristiqnenchevv@gmail.com)

---
