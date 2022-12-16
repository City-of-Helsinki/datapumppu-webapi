
import React, { useState } from 'react'
import ReactDOM from 'react-dom'
import EditableItem from './EditableItem';
import { useTranslation } from 'react-i18next';

export default function AgendaItem(props) {
    const [accordionOpen, setAccordionOpen] = React.useState(false)
    const agenda = props.agenda
    const index = props.index
    const { t } = useTranslation();
    
        return (
        <div className={accordionOpen ? "item item-open" : 'item'}>
            <button className='agendaTitleButton' onClick={() => setAccordionOpen(!accordionOpen)}>
                <span className={
                    accordionOpen
                        ? "icon glyphicon glyphicon-triangle-top"
                        : "icon glyphicon glyphicon-triangle-bottom"
                } />
                {index}. {agenda.subject}
            </button>
            {accordionOpen &&
                <div className='content' >
                    {agenda.attachments?.map((attachment, index) => {
                        return (
                            <div className='attachment' key={'attach' + index}>
                                {t("Attachment")} {index + 1} {''}
                                {attachment.public ?
                                    <a href={attachment.file_uri}>{attachment.name}</a>
                                    : t("Non-public")}
                            </div>
                        )

                    })}
                    {agenda.content?.map((resolution, index) => {
                        return (
                            <EditableItem resolution={resolution} key={'res' + index} />
                        )
                    })}
                </div>
            }
        </div>

    )
}

