import api from "./api";

export const getTasks = async () => {
  const response = await api.get("/api/tasks");
  return response.data;
};

export const createTask = async (task) => {
  const response = await api.post("/api/tasks", task);
};

export const updateTask = async (id, task) => {
  await api.put(`/api/tasks/${id}`, task);
};

export const completeTask = async (id) => {
  await api.put(`/api/tasks/${id}/complete`);
}

export const deleteTask = async (id) => {
  await api.delete(`/api/tasks/${id}`);
};