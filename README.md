# ♻️ Online Social Media Driven Used Item Trading Platform (Razor Pages)

A **mobile-friendly**, web-based platform built using **ASP.NET Core Razor Pages**, designed to enable users to **trade second-hand items** while engaging in **community-driven social media features**. This application promotes sustainable consumption by merging marketplace capabilities with interactive user engagement — fully optimized for desktop **and mobile** devices.

---

## 👥 User Roles and Functionalities

### 🔓 Unregistered Users
- Browse public item listings
- View item details and ratings
- Register and log in to access full features

### 👤 Registered Users
- Post and manage item listings
- Send, receive, and confirm trade offers
- Track trade status (Available, Unavailable, Traded)
- Customize and manage personal profiles
- Interact via social media-style features:
  - Follow/unfollow users
  - Post updates or blogs
  - Like, comment, and share items
- Real-time messaging system using SignalR

### 🛠️ Admin
- Moderate posts, comments, and items
- Manage user accounts and activity
- Oversee all platform operations to ensure safety and quality

---

## 📱 Mobile-Responsive Design

- The UI is fully responsive and designed with **mobile usability** in mind.
- Accessible from smartphones, tablets, and desktops without sacrificing functionality.
- Built using **Bootstrap 5** and adaptive layouts to enhance usability on smaller screens.

---

## 🛒 Core Features

- **Marketplace**: Search, list, and trade used items
- **Trade System**: Offer item-for-item or barter trades
- **Social Media Features**: Like, comment, follow, share
- **User Messaging**: Real-time chat with SignalR
- **Mobile-First UI**: Seamless access across devices

---

## 🔧 Technology Stack

- ASP.NET Core 8.0 (Razor Pages)
- SQL Server (local or hosted)
- SignalR (real-time messaging)
- Bootstrap 5 (responsive UI)
- Entity Framework Core (ORM)

---

## ⚙️ Configuration

Ensure your `appsettings.json` is configured with a valid connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDbName;Trusted_Connection=True;"
  }
}
