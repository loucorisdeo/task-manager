import api from "./api";

export const login = async (request) => {
  const response = await api.post("/api/auth/login", request);
  return response.data;
};

export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("username");
  localStorage.removeItem("role");
};