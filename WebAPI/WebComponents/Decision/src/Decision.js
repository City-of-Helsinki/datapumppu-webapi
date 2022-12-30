
import React, { useEffect } from 'react'
import ReactDOM from 'react-dom'
import { jsPDF } from "jspdf";
import { html2canvas } from "html2canvas";

export default function Decision () {
  // Declare a new state variable, which we'll call "count"
  const [contentHtml, setContentHtml] = React.useState('')
  const [attachments, setAttachments] = React.useState(undefined)
  const [title, setTitle] = React.useState('')

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch('#--API_URL--#/decisions/#--CASE_ID_LABEL--#/#--LANG--#"')
      const decision = await response.json()

      const show = "#--SHOW_CONTENT--#"

      setContentHtml(show.toLocaleLowerCase() === "motion" ? decision.motion : decision.html)
      setAttachments(decision.attachments)
      setTitle(decision.title)
    }
    fetchData()
  }, [setContentHtml])

  const downloadAsPdf = (e) => {
    e.preventDefault()
    let doc = new jsPDF("landscape", 'pt', 'A4');
    doc.html(document.getElementById('element-to-print'), {
      callback: () => {
        doc.save('#--CASE_ID_LABEL--#.pdf');
      }
    })
  }

  const lang = "#--LANG--#".toLowerCase()
  return (
    <div>
      <button onClick={downloadAsPdf}>{lang === 'fi' ? 'Lataa PDF' : 'Ladda PDF'} </button>
      <div id="element-to-print">
        <h3>{title}</h3>
        <div>
          {contentHtml && <div dangerouslySetInnerHTML={{ __html: contentHtml }} />}
        </div>
        <h4>{lang === 'fi' ? "Linkit" : "Hyperlänkar"}</h4>
      </div>
      <div>
        <ul>
          {attachments && attachments.map((attachment, index) =>          
            <li key={index}><a href={attachment.fileURI}>{attachment.title}</a></li>
          )}
        </ul>
      </div>
    </div>
  )
}

