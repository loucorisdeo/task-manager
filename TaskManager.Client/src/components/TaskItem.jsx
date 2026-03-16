const TaskItem = ({ task, onEdit, onComplete, onDelete, isAdmin }) => {
  const {id, title, description, isCompleted} = task;

  return (
    <div className={`task-card ${isCompleted ? "completed" : ""}`}>
      <h3>{title}</h3>
      <p>{description}</p>
      <p>Status: {isCompleted ? "Completed" : "Open"}</p>

      <div className="task-actions">
       <button disabled={isCompleted} onClick={() => onEdit(task)}>
        Edit
      </button>
        {!isCompleted && (
          <button onClick={() => onComplete(id)}>Mark Complete</button>
        )}
          {isAdmin && (
            <button onClick={() => onDelete(id)}>Delete</button>
          )}
      </div>
    </div>
  );
};

export default TaskItem;