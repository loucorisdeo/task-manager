import { useEffect, useState, useCallback } from "react";
import Navbar from "../components/Navbar";
import TaskForm from "../components/TaskForm";
import TaskList from "../components/TaskList";
import * as taskService from "../services/taskService";
import { useAuth } from "../context/AuthContext";

const DashboardPage = () => {
  const [tasks, setTasks] = useState([]);
  const [editingTask, setEditingTask] = useState(null);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const { isAdmin } = useAuth();

  const loadTasks = useCallback(async () => {
    setLoading(true);
    setErrorMessage("");

    try {
      const data = await taskService.getTasks();
      setTasks(data);
    } catch (error) {
      console.error("Failed to load tasks", error);
      setErrorMessage("Failed to load tasks.");
    } finally {
      setLoading(false);
    }
  } , []);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  const handleSaveTask = async (task) => {
    setErrorMessage("");

    try {
      if (editingTask) {
        await taskService.updateTask(editingTask.id, task);
        setEditingTask(null);
      } else {
        await taskService.createTask(task);
      }

      await loadTasks();
    } catch (error) {
      console.error("Failed to save task", error);
      setErrorMessage("Failed to save task.");
    }
  };

  const handleEditTask = (task) => {
    setEditingTask(task);
  };

  const handleCompleteTask = async (id) => {
    setErrorMessage("");

    try {
      await taskService.completeTask(id);
      await loadTasks();
    } catch (error) {
      console.error("Failed to complete task", error);
      setErrorMessage("Failed to complete task.");
    }
  };

 const handleDeleteTask = async (id) => {
  if (!isAdmin) return;

  const confirmed = window.confirm("Are you sure you want to delete this task?");
  if (!confirmed) return;

  setErrorMessage("");

  try {
    await taskService.deleteTask(id);

    if (editingTask?.id === id) {
      setEditingTask(null);
    }

    await loadTasks();
  } catch (error) {
    console.error("Failed to delete task", error);
    setErrorMessage("Failed to delete task.");
  }
};

  return (
    <>
      <Navbar />
      <main className="container">
        {errorMessage && <p>{errorMessage}</p>}

        <TaskForm
          onSave={handleSaveTask}
          editingTask={editingTask}
          onCancelEdit={() => setEditingTask(null)}
        />

        {loading ? (
          <p>Loading tasks...</p>
        ) : (
          <TaskList
            tasks={tasks}
            onEdit={handleEditTask}
            onComplete={handleCompleteTask}
            onDelete={handleDeleteTask}
            isAdmin={isAdmin}
          />
        )}
      </main>
    </>
  );
};

export default DashboardPage;