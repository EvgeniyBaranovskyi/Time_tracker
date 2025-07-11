import React from 'react';
import Modal from 'react-bootstrap/Modal';
import { FormLabel, Row, Col, Button } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import { FormGroup } from '../common/elements/FormGroup';
import { DayOffRequestInput, DayOffRequestReason } from '../../behavior/daysOff/types';
import { useDispatch, useSelector } from 'react-redux';
import { requestDayOff } from '../../behavior/daysOff/actions';
import { required } from '../../behavior/validators';
import { ValidationMessage } from '../common/validation/ValidationMessage';
import { RootState } from '../../behavior/store';

type Props = {
  show: boolean;
  handleClose: () => void;
}

export const NewRequestModal = ({ show, handleClose }: Props) => {
  const currentUserId = useSelector((state: RootState) => state.profile.userInfo?.id);
  const initialValues: DayOffRequestInput = {
    startDate: '',
    finishDate: '',
    reason: DayOffRequestReason.Vacation,
  };
  const dispatch = useDispatch();
  const onSubmit = (values: DayOffRequestInput) => {
    if (currentUserId)
      dispatch(requestDayOff(values, currentUserId));
  };

  return (
    <>
      <Modal show={show} onHide={handleClose} backdrop="static">
        <Formik onSubmit={onSubmit} initialValues={initialValues}>
          <Form>
            <Modal.Header closeButton>
              <Modal.Title>Request a day off</Modal.Title>
            </Modal.Header>
            <Modal.Body>
              <Row>
                <Col>
                  <FormGroup>
                    <FormLabel htmlFor="startDate">Beginning on</FormLabel>
                    <Field name="startDate" type="date" className="form-control" validate={required} />
                    <ValidationMessage fieldName='startDate' />
                  </FormGroup>
                </Col>
              </Row>
              <Row>
                <Col>
                  <FormGroup>
                    <FormLabel htmlFor="finishDate">Ending on</FormLabel>
                    <Field name="finishDate" type="date" className="form-control" validate={required} />
                    <ValidationMessage fieldName='endDate' />
                  </FormGroup>
                </Col>
              </Row>
            </Modal.Body>
            <Modal.Footer>
              <Button type='button' variant='secondary' onClick={handleClose}>Close</Button>
              <Button type='submit' onClick={handleClose}>Save</Button>
            </Modal.Footer>
          </Form>
        </Formik>
      </Modal>
    </>
  );
};