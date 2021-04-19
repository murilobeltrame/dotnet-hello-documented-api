using System;

namespace Documented.Api.Data
{
    public class Todo
    {
        /// <summary>
        /// Identificação unica da tarefa
        /// </summary>
        /// <example>18647c67-be2b-46b9-9be2-49de8b9a3b88</example>
        public Guid Id { get; set; }
        /// <summary>
        /// Descrição da tarefa
        /// </summary>
        /// <example>Fazer exemplo de documentação de APIs</example>
        public string Description { get; set; }
        /// <summary>
        /// Prazo para a conclusão da tarefa
        /// </summary>
        /// <example>2021-04-17T14:22:39.3973797-03:00</example>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Estado atual da tarefa
        /// </summary>
        public Status CurrentStatus { get; set; }
        /// <summary>
        /// Define se a tarefa foi concluida
        /// </summary>
        public bool Finished => CurrentStatus == Status.CANCELLED || CurrentStatus == Status.DONE;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}