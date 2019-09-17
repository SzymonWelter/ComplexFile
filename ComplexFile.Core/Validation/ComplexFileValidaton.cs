using System;
using System.Collections.Generic;
using System.Text;

namespace ComplexFile.Core.Validation
{
    public class ComplexFileValidation : IComplexFileValidaton
    {
        private IValidationFactory _validatonFactory;
        public ComplexFileValidation()
        {
            Setup(new DefaultValidationFactory());
        }

        public ComplexFileValidation(IValidationFactory validatonFactory)
        {
            Setup(validatonFactory);
        }

        public void ValidPath()
        {
            throw new NotImplementedException();
        }

        private void Setup(IValidationFactory validatonFactory)
        {
            _validatonFactory = validatonFactory;
        }
    }
}
