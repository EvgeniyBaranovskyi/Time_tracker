import { combineEpics } from 'redux-observable';
import { profileEpic } from './profile';
import { usersEpic } from './users';
import { userCreationEpic } from './userCreation';
import { daysOffEpic } from './daysOff';
import { userDetailsEpic } from './userDetails';
import { approvalEpic } from './approvals';
import { worktimeEpic } from './worktime';
import { calendarEpic } from './calendar';

export default combineEpics(
  profileEpic,
  usersEpic,
  userCreationEpic,
  userDetailsEpic,
  daysOffEpic,
  approvalEpic,
  worktimeEpic,
  calendarEpic
);