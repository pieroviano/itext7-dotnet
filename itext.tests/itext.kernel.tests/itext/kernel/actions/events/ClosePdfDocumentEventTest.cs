/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Kernel;
using iText.Kernel.Actions;
using iText.Kernel.Actions.Ecosystem;
using iText.Kernel.Actions.Processors;
using iText.Kernel.Actions.Session;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Actions.Events {
    public class ClosePdfDocumentEventTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/actions/";

        [NUnit.Framework.Test]
        public virtual void FieldsTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                ClosePdfDocumentEvent @event = new ClosePdfDocumentEvent(document);
                NUnit.Framework.Assert.AreEqual("close-document-event", @event.GetEventType());
                NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoActionTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    IList<String> forMessages = new List<String>();
                    access.AddProcessor(new ClosePdfDocumentEventTest.TestProductEventProcessor("test-product-1", forMessages)
                        );
                    access.AddProcessor(new ClosePdfDocumentEventTest.TestProductEventProcessor("test-product-2", forMessages)
                        );
                    access.AddEvent(document.GetDocumentIdWrapper(), new ITextTestEvent(document, null, "testing", "test-product-1"
                        ));
                    access.AddEvent(document.GetDocumentIdWrapper(), new ITextTestEvent(document, null, "testing", "test-product-1"
                        ));
                    access.AddEvent(document.GetDocumentIdWrapper(), new ITextTestEvent(document, null, "testing", "test-product-2"
                        ));
                    access.AddEvent(document.GetDocumentIdWrapper(), new ITextTestEvent(document, null, "testing", "test-product-2"
                        ));
                    new ClosePdfDocumentEvent(document).DoAction();
                    NUnit.Framework.Assert.AreEqual(4, forMessages.Count);
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("aggregation message from test-product-1"));
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("aggregation message from test-product-2"));
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("completion message from test-product-1"));
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("completion message from test-product-2"));
                    // check order
                    NUnit.Framework.Assert.IsTrue(forMessages[0].StartsWith("aggregation"));
                    NUnit.Framework.Assert.IsTrue(forMessages[1].StartsWith("aggregation"));
                    NUnit.Framework.Assert.IsTrue(forMessages[2].StartsWith("completion"));
                    NUnit.Framework.Assert.IsTrue(forMessages[3].StartsWith("completion"));
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNKNOWN_PRODUCT_INVOLVED, Count = 2)]
        public virtual void UnknownProductTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    access.AddEvent(document.GetDocumentIdWrapper(), new ITextTestEvent(document, null, "testing", "unknown product"
                        ));
                    NUnit.Framework.Assert.DoesNotThrow(() => new ClosePdfDocumentEvent(document).DoAction());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoActionNullDocumentTest() {
            ClosePdfDocumentEvent closeEvent = new ClosePdfDocumentEvent(null);
            NUnit.Framework.Assert.DoesNotThrow(() => closeEvent.DoAction());
        }

        private class TestProductEventProcessor : ITextProductEventProcessor {
            public readonly IList<String> aggregatedMessages;

            private readonly String processorId;

            public TestProductEventProcessor(String processorId, IList<String> aggregatedMessages) {
                this.processorId = processorId;
                this.aggregatedMessages = aggregatedMessages;
            }

            public virtual void OnEvent(AbstractITextProductEvent @event) {
            }

            // do nothing here
            public virtual String GetProductName() {
                return processorId;
            }

            public virtual void AggregationOnClose(ClosingSession session) {
                aggregatedMessages.Add("aggregation message from " + processorId);
            }

            public virtual void CompletionOnClose(ClosingSession session) {
                aggregatedMessages.Add("completion message from " + processorId);
            }
        }
    }
}
