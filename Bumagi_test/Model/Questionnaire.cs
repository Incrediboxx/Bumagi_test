using System;
using System.Collections.Generic;

namespace Bumagi_test.Model
{

    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Questionnaire : ICloneable
    {

        private string nameField;

        private string codeField;

        private string saveFileNameField;

        private string textTemplateField;

        private QuestionnaireQuestion[] questionsField;

        private DateTime fillDateField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public string SaveFileName
        {
            get
            {
                return this.saveFileNameField;
            }
            set
            {
                this.saveFileNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Question", IsNullable = false)]
        public QuestionnaireQuestion[] Questions
        {
            get
            {
                return this.questionsField;
            }
            set
            {
                this.questionsField = value;
            }
        }

        /// <remarks/>
        public DateTime FillDate
        {
            get
            {
                return this.fillDateField;
            }
            set
            {
                this.fillDateField = value;
            }
        }

        /// <remarks/>
        public string TextTemplate
        {
            get
            {
                return this.textTemplateField;
            }
            set
            {
                this.textTemplateField = value;
            }
        }

        public object Clone()
        {
            List<QuestionnaireQuestion> qstn = new List<QuestionnaireQuestion>();
            foreach (var question in Questions)
                qstn.Add(new QuestionnaireQuestion()
                {
                    Text = question.Text,
                    Mask = question.Mask,
                    Code = question.Code,
                    CheckType = question.CheckType
                });

            return new Questionnaire()
            {
                Name = this.Name,
                Code = this.Code,
                saveFileNameField = this.saveFileNameField,
                TextTemplate = this.TextTemplate,
                Questions = qstn.ToArray()
            };
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QuestionnaireQuestion
    {

        private string textField;

        private string maskField;

        private string checkTypeField;

        private string answerField;

        private string codeField;

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public string Mask
        {
            get
            {
                return this.maskField;
            }
            set
            {
                this.maskField = value;
            }
        }

        /// <remarks/>
        public string CheckType
        {
            get
            {
                return this.checkTypeField;
            }
            set
            {
                this.checkTypeField = value;
            }
        }

        /// <remarks/>
        public string Answer
        {
            get
            {
                return this.answerField;
            }
            set
            {
                this.answerField = value;
            }
        }

        /// <remarks/>
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }
    }


}
