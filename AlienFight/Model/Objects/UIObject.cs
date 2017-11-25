namespace AlienExplorer.Model
{
    /// <summary>
    /// Элемент интерфейса.
    /// </summary>
    public class UIObject : GameObject
    {
        /// <summary>
        /// Тип элемента.
        /// </summary>
        public UIObjectType Type { get; set; }
        /// <summary>
        /// Текст элемента.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// ID элемента.
        /// </summary>
        public int ID { get; set; }
    }
}
