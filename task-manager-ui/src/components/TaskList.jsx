import TaskItem from "./TaskItem";

const TaskList = ({ tasks, onEdit, onComplete, onDelete, isAdmin }) => {
  if (!tasks || tasks.length === 0) {
    return <p>No tasks. Create one to get started!</p>;
  }

  return (
    <div className="task-list">
      {tasks.map((task) => (
        <TaskItem
          key={task.id}
          task={task}
          onEdit={onEdit}
          onComplete={onComplete}
          onDelete={onDelete}
          isAdmin={isAdmin}
        />
      ))}
    </div>
  );
};

export default TaskList;