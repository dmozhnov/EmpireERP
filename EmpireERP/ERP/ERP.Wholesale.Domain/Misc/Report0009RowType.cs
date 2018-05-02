/// <summary>
/// Тип отбора партий для отчета по поставкам
/// </summary>
public enum Report0009RowType
{
    /// <summary>
    /// Партии по дате документа
    /// </summary>
    RowsByDate = 1,

    /// <summary>
    /// Партии по дате проводки
    /// </summary>
    RowsByAcceptenceDate = 2,

    /// <summary>
    /// Партии по дате окончательного согласования
    /// </summary>
    RowsByApprovementDate = 3,

    /// <summary>
    /// Партии по дате документа и дате окончательного согласования
    /// </summary>
    RowsByDateAndApprovementDate = 4,

    /// <summary>
    /// Партии, принятые с расхождением, по дате документа
    /// </summary>
    RowsDivergentByDate = 5,
    
    /// <summary>
    /// Партии, принятые с расхождением, по дате проводки
    /// </summary>
    RowsDivergentByAcceptanceDate = 6,
}