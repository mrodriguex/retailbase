import { useEffect, useState } from "react";
import Login from "./pages/Login";
import Companies from "./pages/Companies";
import Brands from "./pages/Brands";
import Categories from "./pages/Categories";
import Products from "./pages/Products";
import ProductPresentations from "./pages/ProductPresentations";
import Menus from "./pages/Menus";
import Customers from "./pages/Customers";
import Roles from "./pages/Roles";
import Users from "./pages/Users";
import Messages from "./pages/Messages";
import Navbar from "./components/Navbar";
import { isAuthenticated, logout } from "./services/authService";
import { AUTH_EXPIRED_EVENT } from "./services/apiClient";
import { startDataHub, stopDataHub } from "./services/signalrService";

function App() {
  const [loggedIn, setLoggedIn] = useState(isAuthenticated());
  const [page, setPage] = useState("companies");
  const [hubMessage, setHubMessage] = useState("");
  const [authMessage, setAuthMessage] = useState("");

  useEffect(() => {
    function handleAuthExpired(event) {
      stopDataHub();
      setLoggedIn(false);
      setPage("companies");
      setAuthMessage(event?.detail?.message || "Your session expired. Please log in again.");
    }

    window.addEventListener(AUTH_EXPIRED_EVENT, handleAuthExpired);
    return () => window.removeEventListener(AUTH_EXPIRED_EVENT, handleAuthExpired);
  }, []);

  useEffect(() => {
    if (!loggedIn) return;

    let conn;

    (async () => {
      try {
        conn = await startDataHub();

        conn.off("ReceiveUpdate");
        conn.off("ReceiveMessage");

        conn.on("ReceiveUpdate", () => {
          window.dispatchEvent(new CustomEvent("datahub:update"));
        });

        conn.on("ReceiveMessage", (message) => {
          setHubMessage(message || "New message received");
        });
      } catch (error) {
        console.error("SignalR error:", error);
      }
    })();

    return () => {
      if (conn) {
        conn.off("ReceiveUpdate");
        conn.off("ReceiveMessage");
      }
      stopDataHub();
    };
  }, [loggedIn]);

  function handleLogout() {
    logout();
    stopDataHub(); // explicit stop on manual logout
    setLoggedIn(false);
    setPage("companies");
    setAuthMessage("");
  }

  if (!loggedIn) {
    return <Login sessionMessage={authMessage} onLoginSuccess={() => {
      setAuthMessage("");
      setLoggedIn(true);
    }} />;
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar page={page} onNavigate={setPage} onLogout={handleLogout} />
      {page === "companies" && <Companies />}
      {page === "brands" && <Brands />}
      {page === "categories" && <Categories />}
      {page === "products" && <Products />}
      {page === "product-presentations" && <ProductPresentations />}
      {page === "menus" && <Menus />}
      {page === "customers" && <Customers />}
      {page === "roles" && <Roles />}
      {page === "users" && <Users />}
      {page === "messages" && <Messages />}
      {hubMessage && <div className="fixed bottom-0 left-0 right-0 bg-green-500 text-white p-2 text-center">
        {hubMessage}
      </div>}
    </div>
  );
}

export default App;