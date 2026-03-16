import { createContext, useContext, useMemo, useState } from "react";
import * as authService from "../services/authService";

const AuthContext = createContext(undefined);

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(localStorage.getItem("token"));
  const [username, setUsername] = useState(localStorage.getItem("username"));
  const [role, setRole] = useState(localStorage.getItem("role"));

  const loginUser = async (username, password) => {
    const response = await authService.login({ username, password });

    localStorage.setItem("token", response.token);
    localStorage.setItem("username", response.username);
    localStorage.setItem("role", response.role);

    setToken(response.token);
    setUsername(response.username);
    setRole(response.role);
  };


  const logoutUser = () => {
    authService.logout();

    setToken(null);
    setUsername(null);
    setRole(null);
  };

  const value = useMemo(
    () => ({
      token,
      username,
      role,
      isAdmin: role === "Admin",
      isAuthenticated: !!token,
      loginUser,
      logoutUser,
    }),
    [token, username, role]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return context;
};