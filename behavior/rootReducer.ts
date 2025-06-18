import { combineReducers } from 'redux';
import { daysOffReducer } from './daysOff';
import { profileReducer } from './profile';
import { usersReducer } from './users';
import { userCreationReducer } from './userCreation';
import { userDetailsReducer } from './userDetails';
import { approvalsReducer } from './approvals';
import { calendarReducer } from './calendar';
import { worktimeReducer } from './worktime';

export default combineReducers({
  users: usersReducer,
  profile: profileReducer,
  userCreation: userCreationReducer,
  daysOff: daysOffReducer,
  userDetails: userDetailsReducer,
  approvals: approvalsReducer,
  calendar: calendarReducer,
  worktime: worktimeReducer,
});