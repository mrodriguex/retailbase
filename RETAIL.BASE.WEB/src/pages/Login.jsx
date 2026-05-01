import { useState } from "react";
import { login } from "../services/authService";
import projectLogo from "../assets/project_logo.png";

export default function Login({ onLoginSuccess, sessionMessage = "" }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleLogin(e) {
    e.preventDefault();
    setError("");

    if (!username.trim() || !password.trim()) {
      setError("Please enter both username and password.");
      return;
    }

    setLoading(true);
    const result = await login(username, password);
    setLoading(false);

    if (result.success) {
      if (onLoginSuccess) {
        onLoginSuccess(result.token);
      }
    } else {
      setError(result.error || "Login failed.");
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 p-4">
      <form
        onSubmit={handleLogin}
        className="bg-white p-6 sm:p-8 rounded shadow-md w-full max-w-sm"
        noValidate
      >
        <div className="text-center mb-6">
          <img src={projectLogo} alt="Project Logo" className="mx-auto h-16 w-auto" />
        </div>
        <h1 className="text-2xl font-bold mb-6 text-center">Log In</h1>

        {sessionMessage && (
          <div className="mb-4 p-2 bg-amber-100 border border-amber-400 text-amber-800 rounded text-sm">
            {sessionMessage}
          </div>
        )}

        {error && (
          <div className="mb-4 p-2 bg-red-100 border border-red-400 text-red-700 rounded text-sm">
            {error}
          </div>
        )}

        <label className="block text-sm font-medium text-gray-700 mb-1">
          Username
        </label>
        <input
          type="text"
          placeholder="Username"
          className="w-full border border-gray-300 rounded p-2 mb-4 focus:outline-none focus:ring-2 focus:ring-green-500"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          autoComplete="username"
          disabled={loading}
        />

        <label className="block text-sm font-medium text-gray-700 mb-1">
          Password
        </label>
        <input
          type="password"
          placeholder="Password"
          className="w-full border border-gray-300 rounded p-2 mb-6 focus:outline-none focus:ring-2 focus:ring-green-500"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="current-password"
          disabled={loading}
        />

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white font-semibold p-2 rounded transition-colors"
        >
          {loading ? "Logging in..." : "Log In"}
        </button>
      </form>
    </div>
  );
}